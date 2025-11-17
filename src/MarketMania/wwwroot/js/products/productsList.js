function productsList(language)
{
    Alpine.data("productsList", () => ({
        list: {
            items: [],
            pageIndex: 0,
            pageSize: 0,
            totalCount: 0,
            hasNextPage: false
        },
        name: '',
        loading: false,

        loadProducts: async function ()
        {
            this.loading = true;

            try {
                const response = await getProductsAsync(this.name, language);
                const content = await response.json();

                let errorMessage = GetErrorMessage(response.status, content);
                if (errorMessage == null)
                {
                    this.list = content;

                    for (const product of this.list.items)
                    {
                        if (product.images && product.images.length > 0)
                        {
                            const imageResponse = await getImageAsync(product.id, product.images[0].id, language);
                            if (!imageResponse.ok)
                            {
                                const imageContent = await imageResponse.json();
                                errorMessage = GetErrorMessage(imageResponse.status, imageContent);

                                alert(errorMessage);
                            }

                            const blob = await imageResponse.blob();
                            product.imageSource = URL.createObjectURL(blob);
                        }
                        else
                        {
                            product.imageSource = null;
                        }
                    }
                }
                else
                {
                    alert(errorMessage);
                }
            }
            catch (error)
            {
                console.error(error);
            }
            finally
            {
                this.loading = false;
            }
        },

        goToDetails: function (id)
        {
            window.location.href = `/products/details/${id}`;
        }
    }));
}

async function getProductsAsync(name, language)
{
    let url = '/api/products';

    if (name && name.trim() !== '') {
        url += `?name=${encodeURIComponent(name)}`;
    }

    const response = await fetch(url, {
        method: "GET",
        headers: {
            "Accept-Language": language
        }
    });

    return response;
}

async function getImageAsync(productId, imageId, language)
{
    const response = await fetch(`/api/products/${productId}/images/${imageId}`, {
        method: "GET",
        headers: {
            "Accept-Language": language
        }
    });

    return response;
}
