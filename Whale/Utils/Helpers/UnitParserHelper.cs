namespace Whale.Utils.Helpers
{
    public static class UnitParserHelper
    {
        // Helper methods to parse different units (KB, MB, GB, etc.)

        // Parses memory value (e.g., 428KiB) to value and unit.
        public static (float Value, string Unit) ParseMemoryValue(string memoryValue)
        {
            var parts = memoryValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 && float.TryParse(parts[0], out float value))
            {
                return (value, parts[1].Trim());
            }
            else
            {
                return (0, "");
            }
        }

        // Parses network value (e.g., 1.39kB / 0B) to input and output values and units.
        public static (float Input, string InputUnit, float Output, string OutputUnit) ParseNetworkValue(string networkValue)
        {
            var parts = networkValue.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                var (inputValue, inputUnit) = ParseMemoryValue(parts[0]);
                var (outputValue, outputUnit) = ParseMemoryValue(parts[1]);
                return (inputValue, inputUnit, outputValue, outputUnit);
            }
            else
            {
                return (0, "", 0, "");
            }
        }

        // Parses block value (e.g., 0B / 0B) to input and output values and units.
        public static (int Input, string InputUnit, int Output, string OutputUnit) ParseBlockValue(string blockValue)
        {
            var parts = blockValue.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                var (inputValue, inputUnit) = ParseMemoryValue(parts[0]);
                var (outputValue, outputUnit) = ParseMemoryValue(parts[1]);
                return ((int)inputValue, inputUnit, (int)outputValue, outputUnit);
            }
            else
            {
                return (0, "", 0, "");
            }
        }
    }
}
