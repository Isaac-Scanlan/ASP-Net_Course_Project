namespace HPlusSport.API.Models
{
    public class QueryParameters
    {
        const int _maxSize = 100;
        private int _size = 50;

        public int Page { get; set; } = 1;

        public int Size 
        { 
            get 
            { 
                return _size; 
            } 
            set
            {
                _size = Math.Min(_size,value);
            }
        }

        public string SortBy { get; set; } = "Id";
        private string _sortOrder = "asc";
        public string SortOrder 
        { 
            get
            {
                return _sortOrder;
            } 
            set
            {
                if (value.Equals("asc") || value.Equals("desc")) 
                {
                    _sortOrder = value;
                }
            }
        }
    }
}
