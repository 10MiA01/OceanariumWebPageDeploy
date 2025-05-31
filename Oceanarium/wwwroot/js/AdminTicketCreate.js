document.getElementById('orderCode').addEventListener('blur', async function () {
    const code = this.value.trim();
    if (!code) return;

    try {
        const res = await fetch(`?handler=OrderInfo&orderCode=${encodeURIComponent(code)}`);
        const data = await res.json();
        if (data.success) {
            document.querySelector('input[name="newTicket.OrderId"]').value = data.orderId;
            document.querySelector('input[name="newTicket.EventId"]').value = data.eventId;
            document.getElementById('eventName').innerText = data.eventName;
        } else {
            // Not found-eror
            document.querySelector('input[name="newTicket.OrderId"]').value = '';
            document.querySelector('input[name="newTicket.EventId"]').value = '';
            document.getElementById('eventName').innerText = '';
            alert('Order not found or has no tickets');
        }
    } catch (e) {
        console.error(e);
    }
});

document.querySelector('.discount-select').addEventListener('change', async function () {
    const ticketType = this.value.trim();
    const eventId = document.querySelector('input[name="newTicket.EventId"]').value;
    if (!ticketType || !eventId) return;

    try {
        const res = await fetch(`?handler=PriceInfo&ticketType=${encodeURIComponent(ticketType)}&eventId=${eventId}`);
        const data = await res.json();
        if (data.success) {
            document.getElementById('ticketPrice').innerText = data.price.toFixed(2);
        } else {
            // Not found-eror
            document.getElementById('ticketPrice').innerText = '';
            alert('Order not found or has no tickets');
        }
    } catch (e) {
        console.error(e);
    }
});