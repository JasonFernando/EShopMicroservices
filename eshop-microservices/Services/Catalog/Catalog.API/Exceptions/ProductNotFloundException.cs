namespace Catalog.API.Exceptions
{
    public class ProductNotFloundException : Exception
    {
        public ProductNotFloundException() : base("Product not found!")
        {
        }
    }
}
