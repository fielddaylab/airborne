using System;
using System.Runtime.InteropServices;

namespace Airborne.Pollutants {
    /// <summary>
    /// Packed symptom type and severity.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public readonly struct Symptom {
        // Format: STTTTTTT
        private const int TypeMask = (1 << 7) - 1;
        private const int SeverityShift = 7;
        private const int SeverityMask = 0x1;

        private readonly byte m_PackedValue;

        public Symptom(SymptomType type, SymptomSeverity severity) {
            m_PackedValue = (byte) ((((int) severity & SeverityMask) << SeverityShift)
                | ((int) type & TypeMask));
        }

        public SymptomType Type {
            get { return (SymptomType) (m_PackedValue & TypeMask); }
        }

        public SymptomSeverity Severity {
            get { return (SymptomSeverity) ((m_PackedValue >> SeverityShift) & SeverityMask); }
        }
    }

    /// <summary>
    /// Severity of symptom.
    /// </summary>
    public enum SymptomSeverity : byte {
        Minor,
        Major
    }

    /// <summary>
    /// Symptom type.
    /// </summary>
    public enum SymptomType : byte {
        LossOfConsciousness,
        Headache,
        ShortnessOfBreath,
        Dizziness,
        EyeIrritation,
        ChestPain,
        Nausea,
        Confusion,
        Cough,
        AirwayIrritation
    }

    /// <summary>
    /// Symptom type mask.
    /// NOTE: Keep updated with SymptomType.
    /// </summary>
    [Flags]
    public enum SymptomMask : uint {
        LossOfConsciousness = 1 << SymptomType.LossOfConsciousness,
        Headache = 1 << SymptomType.Headache,
        ShortnessOfBreath = 1 << SymptomType.ShortnessOfBreath,
        Dizziness = 1 << SymptomType.Dizziness,
        EyeIrritation = 1 << SymptomType.EyeIrritation,
        ChestPain = 1 << SymptomType.ChestPain,
        Nausea = 1 << SymptomType.Nausea,
        Confusion = 1 << SymptomType.Confusion,
        Cough = 1 << SymptomType.Cough,
        AirwayIrritation = 1 << SymptomType.AirwayIrritation
    }
}