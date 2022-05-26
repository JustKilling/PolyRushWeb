using Microsoft.EntityFrameworkCore;

namespace PolyRushWeb.Helper
{
    public interface IIdentifier
    {
        public int Id { get; set; }
    }
    public static class DetachHelper
    {
        public static void DetachLocal<T>(this DbContext context, T t, int entryId)
            where T : class, IIdentifier
        {
            T? local = context.Set<T>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(entryId));
            if (local == null)
            {
                context.Entry(local).State = EntityState.Detached;
            }
            context.Entry(t).State = EntityState.Modified;
        }
    }
}
