namespace VVTDE.Persistence;

public class DbInitializer
{
    public static void Initialize(VideoDbContext context)
    {
        // Убеждаемся в том, что БД создана
        // Если нет - создадим её на основе контекста
        context.Database.EnsureCreated();
    }
}