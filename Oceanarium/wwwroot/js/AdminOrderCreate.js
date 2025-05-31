
let ticketCount = 1;


function updateTicketIndex(ticketDivUpdate, index) {
    //Id
    ticketDivUpdate.id = `ticketTemplate_${index}`;
    //Quantity
    ticketDivUpdate.querySelector("#ticketNumber").innerText = `Ticket ${index + 1}`;
    ticketDivUpdate.querySelector(".ticket-count-label").setAttribute("for", `ticketQuantity_${index}`);
    const newQuantity = ticketDivUpdate.querySelector(".ticketQuantity");
    newQuantity.id = `ticketQuantity_${index}`;
    newQuantity.name = `_AdminOrder.Tickets[${index}].TicketQuantity`;
    newQuantity.setAttribute("onchange", "updatePrice(this); maxTickets();");

    //Discount type
    ticketDivUpdate.querySelector(".ticket-type-label").setAttribute("for", `discountType_${index}`);
    const newDiscountType = ticketDivUpdate.querySelector(".ticketDiscountType");
    newDiscountType.id = `discountType_${index}`;
    newDiscountType.name = `_AdminOrder.Tickets[${index}].DiscountType`;
    newDiscountType.setAttribute("onchange", "updatePrice(this); categoryCheck();");

    //Price
    ticketDivUpdate.querySelector(".ticket-price-label").setAttribute("for", `ticketsPrice_${index}`);
    const newPrice = ticketDivUpdate.querySelector(".ticketsPrice");
    newPrice.id = `ticketsPrice_${index}`;
    newPrice.name = `_AdminOrder.Tickets[${index}].TicketPrice`;
    newPrice.innerText = "0";

    //update on-click remove
    const removeButton = ticketDivUpdate.querySelector("button");
    removeButton.setAttribute("onclick", "removeTicket(this)");
}

function addTicket() {
    const selectedEvent = document.getElementById("SelectedEventId");
    if (!selectedEvent || !selectedEvent.value) {
        alert("Please select an event first.");
        return;
    }


    //Duplicate ticket form
    const ticketContainer = document.getElementById("ticketContainer");
    const ticketTemplate = document.getElementById("ticketTemplate_0");
    const newTicketDiv = ticketTemplate.cloneNode(true);

    //Update label and index, onchange, ect.
    updateTicketIndex(newTicketDiv, ticketCount);

    //add ticket into a container
    ticketContainer.appendChild(newTicketDiv);

    ticketCount++;

    maxTickets();
    categoryCheck();
    updateTotalPrice();
}


function removeTicket(button) {
    const ticketDiv = button.closest(".ticket");

    const ToRemoveTickets = document.querySelectorAll("#ticketContainer .ticket");
    if (ToRemoveTickets.length === 1) {
        return;
    }

    ticketDiv.remove();
    ticketCount--;

    const ticketsLeft = document.querySelectorAll("#ticketContainer .ticket");

    ticketsLeft.forEach((ticket, index) => {

        updateTicketIndex(ticket, index);
    });

    
    maxTickets();
    categoryCheck();
    updateTotalPrice();
    
}



async function updatePrice(selectElement) {

    if (!selectElement) {
        console.error("selectElement is null or undefined.");
        return;
    }

    const selectedEventId = document.getElementById("SelectedEventId").value;

    // Логируем, какой элемент передается в функцию
    console.log('Element passed to updatePrice:', selectElement);

    // Проверяем, был ли передан элемент
    if (!selectElement) {
        console.error("selectElement is null or undefined.");
        return;
    }

    // Находим родительский элемент с классом .ticket
    const selectedTicket = selectElement.closest(".ticket");

    // Логируем, что найдено
    console.log('Closest ticket element:', selectedTicket);

    // Проверяем, был ли найден родительский элемент .ticket
    if (!selectedTicket) {
        console.error("Closest ticket element not found.");
        return;
    }

    const quantity = parseFloat(selectedTicket.querySelector(".ticketQuantity").value); 
    const discountType = selectedTicket.querySelector(".ticketDiscountType").value;
    const priceInput = selectedTicket.querySelector(".ticketsPrice");

    try {
        const response = await fetch(`/Admin/Orders/Create?handler=UpdatePrice&objEv=${selectedEventId}&discountType=${discountType}`);
        const data = await response.json();

        console.log('Fetched price data:', data);
        if (data && data.price) {
            priceInput.value = (data.price * quantity).toFixed(2);
        } else {
            priceInput.value = 0;
        }

        
    } catch (error) {
        console.error("Error", error);
        priceInput.value = 0;
        
    }

    updateTotalPrice();

}

function updateTotalPrice() {
    const priceElements = document.querySelectorAll(".ticketsPrice");
    let totalPrice = 0;

    priceElements.forEach(priceElement => {
        totalPrice += parseFloat(priceElement.value) || 0;
    });

    document.querySelector('input[name="_AdminOrder.TotalAmount"]').value = totalPrice.toFixed(2);

}

async function updatePriceAll() {
    const selects = document.querySelectorAll(".ticketDiscountType");
    const promises = Array.from(selects).map(select => updatePrice(select));
    await Promise.all(promises);
}


function maxTickets() {
    const selectedEvent = document.getElementById("SelectedEventId");
    const addButton = document.getElementById("addTicketButton");

    if (!selectedEvent || !selectedEvent.value) {
        alert("Please select an event first.");
        return;
    }

    const maxTickets = parseInt(selectedEvent.options[selectedEvent.selectedIndex].dataset.max);
    const tickets = Array.from(document.querySelectorAll(".ticket"));
    if (tickets.length === 0) return;

    const lastTicket = tickets[tickets.length - 1];
    const previousTickets = tickets.slice(0, -1);

    updateTicketStates();

    let totalBeforeLast = 0;
    previousTickets.forEach(ticket => {
        const val = parseInt(ticket.querySelector(".ticketQuantity")?.value) || 0;
        totalBeforeLast += val;
    });

    const remainingForLast = Math.max(1, maxTickets - totalBeforeLast);
    const lastInput = lastTicket.querySelector(".ticketQuantity");

    let lastVal = parseInt(lastInput.value) || 1;
    if (lastVal > remainingForLast) {
        lastInput.value = remainingForLast;
        alert("Total ticket limit reached. Some quantities have been adjusted.");
    }

    const totalSelected = totalBeforeLast + lastVal;

    addButton.disabled = totalSelected >= maxTickets;

    updatePriceAll();
}

function updateTicketStates() {
    const tickets = document.querySelectorAll("#ticketContainer .ticket");

    tickets.forEach((ticket, index) => {
        const qty = ticket.querySelector(".ticketQuantity");

        const isLast = index === tickets.length - 1;

        if (qty) qty.readOnly = !isLast;
    });
}

function categoryCheck() {
    const tickets = document.querySelectorAll("#ticketContainer .ticket");

    const selectedCategories = Array.from(tickets)
        .map(ticket => ticket.querySelector(".ticketDiscountType")?.value)
        .filter(val => val);

    tickets.forEach((ticket, index) => {
        const select = ticket.querySelector(".ticketDiscountType");
        if (!select) return;

        const currentValue = select.value;

        Array.from(select.options).forEach(option => {
            if (option.value === currentValue || option.value === "") {
                option.disabled = false;
            } else {
                option.disabled = selectedCategories.includes(option.value);
            }
        });
    });
}