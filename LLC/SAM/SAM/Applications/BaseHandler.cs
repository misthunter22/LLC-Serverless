using Amazon.DynamoDBv2.DocumentModel;
using SAM.DI;

namespace SAM.Applications
{
    public abstract class BaseHandler
    {
        protected ILLCData Service { get; set; }

        protected BaseHandler()
        {
            Service = new ILLCDataImpl();
        }

        protected void AddToDocument(Document item, string key, object value)
        {
            if (value != null)
            {
                if (value is int || value is int?)
                    item.Add(key, (int)value);

                else if (value is decimal || value is decimal?)
                    item.Add(key, (decimal)value);

                else if (value is long || value is long?)
                    item.Add(key, (long)value);

                else if (value is double || value is double?)
                    item.Add(key, (double)value);

                else
                    item.Add(key, value.ToString());
            }
        }
    }
}
