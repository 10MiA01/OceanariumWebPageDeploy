//Ticket functions

let ticketCount = 1;
function addTicket() {
    const selectedEvent = document.getElementById("SelectedEventId");
    if (!selectedEvent || !selectedEvent.value) {
        alert("Please select an event first.");
        return;
    }

    const maxTickets = parseInt(selectedEvent.options[selectedEvent.selectedIndex].dataset.max);
    const ticketContainer = document.getElementById("ticketContainer");
    const currentTickets = ticketContainer.querySelectorAll(".ticket").length;

    if (currentTickets >= maxTickets) {
        alert("No more tickets available for this event.");
        return;
    }



    //search for a ticket form
    const ticketTemplate = ticketContainer.querySelector(".ticket");
    const newTicketDiv = ticketTemplate.cloneNode(true);

    //index for a new ticket
    newTicketDiv.setAttribute("data-ticket-index", ticketCount);
    newTicketDiv.querySelector("label").innerText = `Ticket ${ticketCount + 1}`;

    //default selector
    const newSelect = newTicketDiv.querySelector(".ticketDiscountType")
    newSelect.name = `TicketsOrder.Tickets[${ticketCount}].DiscountType`;
    newSelect.id = `DiscountType_${ticketCount}`;
    newSelect.setAttribute("onchange", "updatePrice(this)");


    //update Price
    const priceSpan = newTicketDiv.querySelector(".eventPrice");
    priceSpan.innerText = "0";

    //update on-click remove
    const removeButton = newTicketDiv.querySelector("button");
    removeButton.setAttribute("onclick", "removeTicket(this)");

    //add ticket into a container
    ticketContainer.appendChild(newTicketDiv);

    ticketCount++;
    updateTotalPrice();
    updateAddButtonState();
}

function removeTicket(button) {

    const ticketDiv = button.closest(".ticket");
    const ToRemoveTickets = document.querySelectorAll("#ticketContainer .ticket");


    if (ToRemoveTickets.length === 1) {
        return;
    }
    ticketDiv.remove();

    const tickets = document.querySelectorAll("#ticketContainer .ticket");


    tickets.forEach((ticket, index) => {
        ticket.setAttribute("data-ticket-index", index);
        const label = ticket.querySelector("label");
        label.innerText = `Ticket ${index + 1}`;

        const select = ticket.querySelector(".ticketDiscountType");
        select.name = `TicketsOrder.Tickets[${index}].DiscountType`;
        select.id = `DiscountType_${index}`;
    });

    ticketCount = tickets.length;
    updateTotalPrice();
    updateAddButtonState();
}

async function updatePrice(selectElement) {
    const selectedEventId = document.getElementById("SelectedEventId").value;
    const discountType = selectElement.value;

    const ticketDiv = selectElement.closest(".ticket");
    const priceSpan = ticketDiv.querySelector(".eventPrice");

    try {
        const response = await fetch(`/Tickets?handler=UpdatePrice&objEv=${selectedEventId}&discountType=${discountType}`);
        const data = await response.json();

        console.log('Fetched price data:', data);
        if (data && data.price) {
            priceSpan.innerText = data.price;
        } else {
            priceSpan.innerText = "0";
        }


        updateTotalPrice();

        return data.price;
    } catch (error) {
        console.error("Error", error);
        priceSpan.innerText = "0";
        updateTotalPrice();
        return 0;
    }

}

async function updatePriceAll() {
    const selects = document.querySelectorAll(".ticketDiscountType");
    const promises = Array.from(selects).map(select => updatePrice(select));
    await Promise.all(promises);
    updateTotalPrice();
}

function updateTotalPrice() {
    const priceElements = document.querySelectorAll(".eventPrice");
    let totalPrice = 0;

    priceElements.forEach(priceElement => {
        totalPrice += parseFloat(priceElement.innerText) || 0;
    });

    document.getElementById("totalPrice").innerText = totalPrice.toFixed(2);

}

function updateAddButtonState() {
    const addButton = document.getElementById("addTicketButton");
    addButton.disabled = ticketCount >= 10;
}
