using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Horizon.MVC.Pages
{
    public class ScholarshipModel : PageModel
    {
        private readonly ILogger<ScholarshipModel> _logger;

        public ScholarshipModel(ILogger<ScholarshipModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public string FullName { get; set; } = string.Empty;

        [BindProperty]
        public string AcademicLevel { get; set; } = string.Empty;

        [BindProperty]
        public string ScholarshipType { get; set; } = string.Empty;

        [BindProperty]
        public string Gpa { get; set; } = string.Empty;

        [BindProperty]
        public string Statement { get; set; } = string.Empty;

        public bool IsSubmitted { get; set; }

        public void OnGet()
        {
            IsSubmitted = false;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Log the application securely
            _logger.LogInformation("Scholarship Application Submitted: {Name}, Type: {Type}, Level: {Level}", 
                FullName, ScholarshipType, AcademicLevel);
            
            IsSubmitted = true;
            return Page();
        }
    }
}
