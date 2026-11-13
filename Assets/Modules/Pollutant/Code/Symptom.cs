using System;
using System.Runtime.InteropServices;
using BeauUtil;
using BeauUtil.Debugger;

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

        #region Utilities

        /// <summary>
        /// Parses a symptom from the given slice.
        /// Major symptoms should append "-major" to the end.
        /// </summary>
        static public Symptom Parse(StringSlice slice) {
            if (slice.IsEmpty) {
                return default;
            }

            SymptomType type;
            SymptomSeverity severity;

            int dashIndex = slice.IndexOf('-');
            Assert.True(dashIndex < 0 || dashIndex > 0, "Invalid symptom format '{0}' - dash must not be at start");
            if (dashIndex > 0) {
                StringSlice typeSlice = slice.Substring(0, dashIndex);
                StringSlice severitySlice = slice.Substring(dashIndex + 1);
                Assert.True(!severitySlice.IsEmpty, "Invalid symptom format '{0}' - dash must not be at end");
                type = ParseType(typeSlice);
                severity = ParseSeverity(severitySlice);
            } else {
                severity = SymptomSeverity.Minor;
                type = ParseType(slice);
            }

            return new Symptom(type, severity);
        }

        /// <summary>
        /// Parses a type from the given string slice.
        /// Case-insensitive.
        /// </summary>
        static public SymptomType ParseType(StringSlice slice) {
            StringHash32 fastHash = StringHash32.FastCaseInsensitive(slice);
            if (fastHash.IsEmpty) {
                return SymptomType.None;
            }

            int index = ArrayUtils.IndexOf(SymptomNames.Type_Hashes, fastHash);
            Assert.True(index >= 0, "Invalid symptom type '{0}'", slice);
            return (SymptomType) index;
        }

        /// <summary>
        /// Parses a severity from the given string slice.
        /// Accepted values are "major", "minor", or empty.
        /// Case-insensitive.
        /// </summary>
        static public SymptomSeverity ParseSeverity(StringSlice slice) {
            StringHash32 fastHash = StringHash32.FastCaseInsensitive(slice);
            if (fastHash.IsEmpty || fastHash == SymptomNames.Severity_Minor) {
                return SymptomSeverity.Minor;
            }
            if (fastHash == SymptomNames.Severity_Major) {
                return SymptomSeverity.Major;
            }
            Assert.Fail("Invalid symptom severity '{0}'", slice);
            return SymptomSeverity.Minor;
        }

        #endregion // Utilities
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
        None,
        Fainting,
        Headache,
        ShortnessOfBreath,
        Dizziness,
        EyeIrritation,
        ChestPain,
        Nausea,
        Confusion,
        Cough,
        AirwayIrritation,
        Fatigue
    }

    /// <summary>
    /// Symptom type mask.
    /// NOTE: Keep updated with SymptomType.
    /// </summary>
    [Flags]
    public enum SymptomMask : uint {
        None = 0,
        Fainting = 1 << SymptomType.Fainting,
        Headache = 1 << SymptomType.Headache,
        ShortnessOfBreath = 1 << SymptomType.ShortnessOfBreath,
        Dizziness = 1 << SymptomType.Dizziness,
        EyeIrritation = 1 << SymptomType.EyeIrritation,
        ChestPain = 1 << SymptomType.ChestPain,
        Nausea = 1 << SymptomType.Nausea,
        Confusion = 1 << SymptomType.Confusion,
        Cough = 1 << SymptomType.Cough,
        AirwayIrritation = 1 << SymptomType.AirwayIrritation,
        Fatigue = 1 << SymptomType.Fatigue
    }

    static public class SymptomNames {
        static public readonly StringHash32 Severity_Major = StringHash32.CaseInsensitive("Major");
        static public readonly StringHash32 Severity_Minor = StringHash32.CaseInsensitive("Minor");

        // keep this in parallel with SymptomType
        static public readonly StringHash32[] Type_Hashes = new StringHash32[] {
            StringHash32.CaseInsensitive("None"),
            StringHash32.CaseInsensitive("Fainting"),
            StringHash32.CaseInsensitive("Headache"),
            StringHash32.CaseInsensitive("ShortnessOfBreath"),
            StringHash32.CaseInsensitive("Dizziness"),
            StringHash32.CaseInsensitive("EyeIrritation"),
            StringHash32.CaseInsensitive("ChestPain"),
            StringHash32.CaseInsensitive("Nausea"),
            StringHash32.CaseInsensitive("Confusion"),
            StringHash32.CaseInsensitive("Cough"),
            StringHash32.CaseInsensitive("AirwayIrritation"),
            StringHash32.CaseInsensitive("Fatigue"),
        };
    }
}