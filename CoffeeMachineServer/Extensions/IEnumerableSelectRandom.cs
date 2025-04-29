
namespace CoffeeMachineServer.Extensions;

public static class EnumerableExtensions
{
    private static readonly Random Random = new Random();

    public static T SelectRandom<T>(this IEnumerable<T> sequence)
    {
        ArgumentNullException.ThrowIfNull(sequence);

        if (!sequence.Any())
        {
            throw new ArgumentException("The sequence is empty.");
        }

        //optimization for ICollection<T>
        if (sequence is ICollection<T>)
        {
            ICollection<T> col = (ICollection<T>)sequence;
            return col.ElementAt(Random.Next(col.Count));
        }

        var count = 1;
        var selected = default(T);

        foreach (var element in sequence)
        {
            if (Random.Next(count++) == 0)
            {
                //Select the current element with 1/count probability
                selected = element;
            }
        }

        return selected;
    }

}