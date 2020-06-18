using ContosoUniversity.Misc;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly Data.SchoolContext _context;

        public IndexModel(Data.SchoolContext context)
        {
            _context = context;
        }

        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public PaginatedList<Student> Students { get; set; }

        public async Task OnGetAsync(string sortOrder, string searchString, string currentFilter, int? pageIndex)
        {
            CurrentSort = sortOrder;

            NameSort = string.IsNullOrEmpty(sortOrder) ? "LastName_desc" : "";
            DateSort = sortOrder == "EnrollmentDate" ? "EnrollmentDate_desc" : "EnrollmentDate";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<Student> studentsIQ = from s in _context.Students
                                             select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                studentsIQ = studentsIQ.Where(s => s.LastName.Contains(searchString) || s.FirstMidName.Contains(searchString));
            }

            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "LastName";
            }

            bool descending = false;
            if (sortOrder.EndsWith("_desc"))
            {
                sortOrder = sortOrder[0..^5];
                descending = true;
            }

            if (descending)
            {
                studentsIQ = studentsIQ.OrderByDescending(s => EF.Property<object>(s, sortOrder));
            }
            else
            {
                studentsIQ = studentsIQ.OrderBy(s => EF.Property<object>(s, sortOrder));
            }

            /* studentsIQ = sortOrder switch
             {
                 "LastName_desc" => studentsIQ.OrderByDescending(s => s.LastName),
                 "EnrollmentDate" => studentsIQ.OrderBy(s => s.EnrollmentDate),
                 "EnrollmentDate_desc" => studentsIQ.OrderByDescending(s => s.EnrollmentDate),
                 _ => studentsIQ.OrderBy(s => s.LastName)
             };*/

            int pageSize = 3;
            Students = await PaginatedList<Student>.CreateAsync(studentsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}