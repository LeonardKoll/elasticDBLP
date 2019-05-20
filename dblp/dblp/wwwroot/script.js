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
        computed: {
            search: {
                get() {
                    return this.searchterm;
                },
                set(value) {
                    this.searchterm = value;
                    connection.invoke('Search', value);
                }
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

    // connection.invoke("SendMessage", user, message).catch(err => console.error(err.toString()));
};