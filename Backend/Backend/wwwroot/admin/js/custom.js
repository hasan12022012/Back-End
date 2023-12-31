﻿$(function () {

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

    $(document).on("click", ".dlt", function (e) {
        e.preventDefault();
        let productId = $(this).attr("book-id");
        let deletedElement = $(this).parent();

        $.ajax({
            url: "/Admin/Author/DeleteProduct",
            type: "Post",
            data: {
                productId: productId
            },
            success: function (res) {
                if (res) {
                    $(deletedElement).remove();
                } else {
                    alert("Author books must be min 1")
                }
            }
        })
    })

    $(document).on("click", ".dltBlog", function (e) {
        e.preventDefault();
        let blogId = $(this).attr("blog-id");
        let deletedElement = $(this).parent();

        $.ajax({
            url: "/Admin/BlogCategory/DeleteBlog",
            type: "Post",
            data: {
                blogId: blogId
            },
            success: function (res) {
                if (res) {
                    $(deletedElement).remove();
                } else {
                    alert("Category blogs must be min 1")
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

    $(document).on("click", "#deleteAuthor", function (e) {
        e.preventDefault();
        let genreId = $(this).attr("author-id");
        let deletedElement = $(this).parent().parent();

        $.ajax({
            url: "/Admin/Author/Delete",
            type: "Post",
            data: {
                id: genreId
            },
            success: function () {
                $(deletedElement).remove();
            }
        })
    })

    $(document).on("click", "#deleteBlog", function (e) {
        e.preventDefault();
        let blogId = $(this).attr("blog-id");
        let deletedElement = $(this).parent().parent();

        $.ajax({
            url: "/Admin/Blog/Delete",
            type: "Post",
            data: {
                id: blogId
            },
            success: function () {
                $(deletedElement).remove();
            }
        })
    })

    $(document).on("click", "#deleteCategory", function (e) {
        e.preventDefault();
        let categoryId = $(this).attr("category-id");
        let deletedElement = $(this).parent().parent();

        $.ajax({
            url: "/Admin/BlogCategory/Delete",
            type: "Post",
            data: {
                id: categoryId
            },
            success: function () {
                $(deletedElement).remove();
            }
        })
    })

    $(document).on("click", "#deleteTag", function (e) {
        e.preventDefault();
        let tagId = $(this).attr("tag-id");
        let deletedElement = $(this).parent().parent();

        $.ajax({
            url: "/Admin/Tag/Delete",
            type: "Post",
            data: {
                id: tagId
            },
            success: function () {
                $(deletedElement).remove();
            }
        })
    })

    $(document).on("click", "#deleteSlider", function (e) {
        e.preventDefault();
        let sliderId = $(this).attr("slider-id");
        let deletedElement = $(this).parent().parent();

        $.ajax({
            url: "/Admin/Slider/Delete",
            type: "Post",
            data: {
                id: sliderId
            },
            success: function () {
                $(deletedElement).remove();
            }
        })
    })

    $(document).on("click", "#deleteAbout", function (e) {
        e.preventDefault();
        let aboutId = $(this).attr("about-id");
        let deletedElement = $(this).parent().parent();

        $.ajax({
            url: "/Admin/About/Delete",
            type: "Post",
            data: {
                id: aboutId
            },
            success: function () {
                $(deletedElement).remove();
            }
        })
    })
});