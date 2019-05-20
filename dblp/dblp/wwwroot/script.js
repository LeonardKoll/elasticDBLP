window.onload = function () {

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/searchHub")
        .build();

    var app = new Vue({
        el: '#app',
        data: {
            hits: [],
            searchterm:""
        },
        methods: {
            search : function () {
                connection.invoke('Search', this.searchterm);
            }
        }
    });

    connection.on("ReceiveResults", (hits) => {
        app.hits = JSON.parse(hits);
    });

    connection.error = function (error) {
        console.log('SignalR error: ' + error);
    };

    connection.start();
};