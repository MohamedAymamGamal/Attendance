using Job.Helper;
using System.Linq;
namespace Job.Extension
{
    public static class Pagination
    {
        public static IEnumerable<TSource> Paginate<TSource>(this IEnumerable<TSource> sources, PaginationParams param)
        {



            if (sources == null)
                throw new ArgumentNullException($"{nameof(sources)}");

            if (param.PageNumber <= 0)
                throw new ArgumentException($"{nameof(param.PageNumber)}");

            if (param.PageSize <= 0)
            {
                throw new ArgumentException($"{nameof(param.PageSize)}");
            }

            if (!sources.Any())
            {
                return Enumerable.Empty<TSource>();
            }


            return sources.Skip((param.PageNumber - 1) * param.PageSize).Take(param.PageSize);
        }
    }
}
