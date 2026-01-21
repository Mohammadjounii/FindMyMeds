const connection = new signalR.HubConnectionBuilder()
    .withUrl("/medicationHub")
    .withAutomaticReconnect()
    .build();

connection.start()
    .then(() => console.log("✅ SignalR connected (Pharmacy Details)"))
    .catch(err => console.error("❌ SignalR error:", err));

connection.on("MedicationUpdated", function () {

    // This page is read-only → safe to reload
    if (document.getElementById("pharmacyInventoryTable")) {
        location.reload();
    }
});
