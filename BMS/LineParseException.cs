using System;
using System.Collections.Generic;
using System.Text;

namespace BMS {
    public class LineParseException: Exception {
        public int Line { get; private set; }

        public LineParseException(int line, Exception innerException) :
            base($"Error while parsing line {line}: {innerException.Message}", innerException) {
            Line = line;
        }
    }
}
