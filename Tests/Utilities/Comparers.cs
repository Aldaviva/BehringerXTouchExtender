using Melanchall.DryWetMidi.Core;

namespace Tests.Utilities;

/*
 * Sadly, MidiEvent classes in DryWetMidi don't override object.Equals(object), so comparing instances with the same property values always returns false.
 * This makes assertions in automated tests require an additional comparer, or writing a predicate function for each assertion.
 */

internal class NoteEventComparer: IEqualityComparer<NoteEvent> {

    public static readonly NoteEventComparer Instance = new();

    public bool Equals(NoteEvent? x, NoteEvent? y) {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.NoteNumber.Equals(y.NoteNumber) && x.Velocity.Equals(y.Velocity) && x.EventType.Equals(y.EventType);
    }

    public int GetHashCode(NoteEvent obj) {
        return HashCode.Combine(obj.NoteNumber, obj.Velocity, obj.EventType);
    }

}

internal class ControlChangeEventComparer: IEqualityComparer<ControlChangeEvent> {

    public static readonly ControlChangeEventComparer Instance = new();

    public bool Equals(ControlChangeEvent? x, ControlChangeEvent? y) {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.ControlNumber.Equals(y.ControlNumber) && x.ControlValue.Equals(y.ControlValue);
    }

    public int GetHashCode(ControlChangeEvent obj) {
        return HashCode.Combine(obj.ControlNumber, obj.ControlValue);
    }

}

internal class SysExEventComparer: IEqualityComparer<SysExEvent> {

    public static readonly SysExEventComparer Instance = new();

    public bool Equals(SysExEvent? x, SysExEvent? y) {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Data.SequenceEqual(y.Data) && x.EventType.Equals(y.EventType);
    }

    public int GetHashCode(SysExEvent obj) {
        return HashCode.Combine(obj.Data, obj.EventType);
    }

}