using Microsoft.ML;

namespace MyAcademy_JWT.Models.MLNetViewModels
{
    public class MLModelState
    {
        public ITransformer? Model { get; private set; }
        public DataViewSchema? Schema { get; private set; }

        private readonly object _lock = new();

        public void UpdateModel(ITransformer model, DataViewSchema schema)
        {
            lock (_lock)
            {
                Model = model;
                Schema = schema;
            }
        }
    }
}
