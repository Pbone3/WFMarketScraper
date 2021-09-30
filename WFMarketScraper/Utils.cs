using System.Text;

namespace WFMarketScraper
{
    public static class Utils
    {
        public static string[] PrimeWarframeNames = new string[31] {
            "ash_prime", "atlas_prime", "chroma_prime", "ember_prime", "equinox_prime", "frost_prime", "gara_prime",
            "hydroid_prime", "inaros_prime", "ivara_prime", "limbo_prime", "loki_prime", "mag_prime", "mesa_prime", "mirage_prime",
            "nekros_prime", "nezha_prime", "nidus_prime", "nova_prime", "nyx_prime", "oberon_prime", "octavia_prime", "rhino_prime",
            "saryn_prime", "titania_prime", "trinity_prime", "valkyr_prime", "vauban_prime", "volt_prime", "wukong_prime", "zephyr_prime",
        };

        public static void AppendMany(this StringBuilder builder, char c, int amount)
        {
            for (int i = 0; i < amount; i++)
                builder.Append(c);
        }

        public static string[] GetPartNames(string wf) => new string[5] {
            $"{wf}_blueprint",
            $"{wf}_neuroptics",
            $"{wf}_chassis",
            $"{wf}_systems",
            $"{wf}_set"
        };

        public static string FormatName(string name) => name.Replace('_', ' ');
    }
}
