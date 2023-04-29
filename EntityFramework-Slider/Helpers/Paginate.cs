using System.Security.Policy;

namespace EntityFramework_Slider.Helpers
{
    public class Paginate<T>
    {
        public List<T> Datas { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public Paginate(List<T> datas, int currentPage, int totalPage )
        {
            Datas = datas;
            CurrentPage = currentPage;
            TotalPages = totalPage;
        }

        public bool HasPrevius                     // Mueyyen sert daxilinde ancaq get edeceyik(yoxlayacayiq)//
        {
            get
            {
                return CurrentPage > 1;

            }
        }

        public bool HasNext
        {
            get
            {
                return CurrentPage < TotalPages;
            }
        }
    }
}
