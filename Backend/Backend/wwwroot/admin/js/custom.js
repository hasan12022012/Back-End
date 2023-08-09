$(function () {

    $(document).on("click", ".btn-danger", function (e) {
        e.preventDefault();
        let imageId = $(this).attr("product-id");
        let deletedElement = $(this).parent();
        let data = { id: imageId };

        $.ajax({
            url: "/Admin/Product/DeleteProductImage",
            type: "Post",
            data: data,
            success: function (res) {
                if (res) {
                    $(deletedElement).remove();
                } else {
                    alert("Product images must be min 1")
                }
            }
        })
    })

    $(document).on("click", "#delete", function (e) {
        e.preventDefault();
        let productId = $(this).attr("product-id");
        let deletedElement = $(this).parent().parent();

        $.ajax({
            url: "/Admin/Product/Delete",
            type: "Post",
            data: {
                id: productId
            },
            success: function () {
                $(deletedElement).remove();
            }
        })
    })

    $(document).on("click", "#deleteGenre", function (e) {
        e.preventDefault();
        let genreId = $(this).attr("genre-id");
        let deletedElement = $(this).parent().parent();

        $.ajax({
            url: "/Admin/Genre/Delete",
            type: "Post",
            data: {
                id: genreId
            },
            success: function () {
                $(deletedElement).remove();
            }
        })
    })
});