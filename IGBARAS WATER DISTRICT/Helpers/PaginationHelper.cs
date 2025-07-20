using System;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    public class PaginationHelper
    {
        public int PageSize { get; private set; }
        public int CurrentPage { get; private set; }
        public int TotalRecords { get; private set; }
        public int TotalPages => PageSize <= 0 ? 1 : (int)Math.Ceiling((double)TotalRecords / PageSize);
        public bool IsPagingEnabled => PageSize > 0;

        public PaginationHelper(int pageSize = 20)
        {
            PageSize = pageSize;
            CurrentPage = 0;
        }

        public void SetPageSize(int newSize)
        {
            PageSize = newSize;
            CurrentPage = 0;
        }

        public void SetTotalRecords(int count)
        {
            TotalRecords = count;
        }

        public void NextPage()
        {
            if (CurrentPage < TotalPages - 1)
                CurrentPage++;
        }

        public void PreviousPage()
        {
            if (CurrentPage > 0)
                CurrentPage--;
        }

        public int GetOffset()
        {
            return PageSize * CurrentPage;
        }

        public void Reset()
        {
            CurrentPage = 0;
        }

        public string GetPageInfo()
        {
            return IsPagingEnabled ? $"Page {CurrentPage + 1} of {TotalPages}" : "Showing all records";
        }
    }
}
