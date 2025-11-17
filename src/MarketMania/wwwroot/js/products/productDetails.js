function productDetails(language)
{
    Alpine.data("productDetails", () => ({
        product: null,
        images: [],
        loading: false,

        get: async function (id)
        {
            this.loading = true;

            try
            {
                const response = await getAsync(id, language);
                const content = await response.json();

                let errorMessage = GetErrorMessage(response.status, content);
                if (errorMessage == null)
                {
                    this.product = content;

                    if (this.product.images && this.product.images.length > 0)
                    {
                        for (const image of this.product.images)
                        {
                            const imageResponse = await getImageAsync(id, image.id, language);
                            if (!imageResponse.ok)
                            {
                                const imageContent = await imageResponse.json();
                                errorMessage = GetErrorMessage(imageResponse.status, imageContent);

                                alert(errorMessage);
                            }

                            const blob = await imageResponse.blob();
                            image.source = URL.createObjectURL(blob);

                            this.images.push(image);
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
            finally {
                this.loading = false;
            }
        }
    }));
}

async function getAsync(id, language)
{
    const response = await fetch(`/api/products/${id}`, {
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