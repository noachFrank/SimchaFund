$(() => {
    console.log("new simcha")

    $("#new-simcha").on("click", function () {
    console.log("new simcha hit")

        new bootstrap.Modal($('.modal')[0]).show();

    })
})