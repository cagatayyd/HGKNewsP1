// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Sayfanın yüklendiğinde URL'den haber ID'sini al

//$(document).ready(function () {
//    $.ajax({
//        url: '/News/List',
//        type: 'POST',
//        dataType: 'json',
//        success: function (data) {
//            $("#Title").html(data.Title);
//            $("#Author").html(data.Author);
//            $("#Category").html(data.Category);
//            $("#CreatedDate").html(data.CreatedDate);
//            $("#NewsDate").html(data.NewsDate);
//            $("#Country").html(data.Country);
//            console.log(data)
//        }
//    });
//});

new DataTable('#example', {
    initComplete: function () {
        this.api()
            .columns([1,2,3,4,5])
            .every(function () {
                let column = this;

                // Create select element
                let select = document.createElement('select');
                select.id = 'select-table';
                select.add(new Option('Select ' + column.header().textContent));
                select[0].value = '';
                column.footer().replaceChildren(select);
                // Apply listener for user change in value
                select.addEventListener('change', function () {
                    var val = DataTable.util.escapeRegex(select.value);

                    column
                        .search(val ? '^' + val + '$' : '', true, false)
                        .draw();
                });
                // Add list of options
                column
                    .data()
                    .unique()
                    .sort()
                    .each(function (d, j) {
                        select.add(new Option(d));
                });
            });
    }
});
document.getElementById('title').innerHTML = "";
document.getElementById("example_filter").classList.add("mb-3");
const selectTables = document.querySelectorAll("#select-table");
selectTables.forEach(function (element) {
    element.classList.add("form-select");
});