using Melanchall.DryWetMidi.Core;

namespace Tests.Helpers;

/*
 * This class is detected by FakeItEasy at runtime using reflection to scan the Tests assembly.
 * https://fakeiteasy.github.io/docs/8.3.0/formatting-argument-values/#how-does-fakeiteasy-find-argument-value-formatters
 */
public class SysExEventFormatter: ArgumentValueFormatter<SysExEvent> {

    protected override string GetStringValue(SysExEvent argumentValue) {
        return $"{argumentValue} with data {Convert.ToHexString(argumentValue.Data)}";
    }

}