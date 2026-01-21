const connection = new signalR.HubConnectionBuilder()
    .withUrl("/medicationHub")
    .withAutomaticReconnect()
    .build();

connection.start()
    .then(() => console.log("✅ SignalR connected (Search Page)"))
    .catch(err => console.error("❌ SignalR error:", err));

// 🔔 When inventory changes anywhere

connection.on("MedicationUpdated", function () {
    if (!document.getElementById("qInput")) return;

    const q = document.getElementById("qInput").value;

    if (!q || q.trim().length === 0) return;

    if (typeof triggerLiveSearch === "function") {
        triggerLiveSearch();
    }
});
