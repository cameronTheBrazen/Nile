/*
 * ITSE 1430
 */
namespace Nile.Stores
{
    /// <summary>Base class for product database.</summary>
    public abstract class ProductDatabase : IProductDatabase
    {        
        /// <inheritdoc />
        public virtual Product Add ( Product product )
        {
           
            if (product != new Product())
                throw new ArgumentException("Invalid product") ;
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            ObjectValidator.Validate(product);
            var existing = Get(product.Id);
            if (existing != null)
            {
                throw new ArgumentException("Product already exists");
            }
            //Emulate database by storing copy
            return AddCore(product);
        }

        /// <inheritdoc />
        public virtual Product Get ( int id )
        {
            
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than 0");

            try
            {
                return GetCore(id);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Get failed", e);
            };
            
        }

        /// <inheritdoc />
        public virtual IEnumerable<Product> GetAll ()
        {
            return GetAllCore();
        }

        /// <inheritdoc />
        public virtual void Remove ( int id )
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than 0");

          

            

            RemoveCore(id);
        }

        /// <inheritdoc />
        public virtual Product Update ( Product product )
        {
            
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            if (product.Id < 0) { throw new InvalidDataException("Id cannot be less than 0!"); }
            if (String.IsNullOrEmpty(product.Name))
            { throw new InvalidDataException("Product Name Cannot be empty!"); }
            ObjectValidator.Validate(product);
            //Get existing product
            var existing = GetCore(product.Id);
            if (existing != product)
            {
                throw new Exception("Name cannot be modified!");
            }
            
            
                return UpdateCore(existing, product);
            

        }

        #region Protected Members

        protected abstract Product GetCore( int id );

        protected abstract IEnumerable<Product> GetAllCore();

        protected abstract void RemoveCore( int id );

        protected abstract Product UpdateCore( Product existing, Product newItem );

        protected abstract Product AddCore( Product product );
        #endregion
    }
}
