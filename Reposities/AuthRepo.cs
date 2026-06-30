using Job.Data;

namespace Job.Reposities
{
    public class AuthRepo
    {
        private readonly ApplicationDbContext _context;

        public AuthRepo(ApplicationDbContext context)
        {
            _context = context;
        }



    }
}
