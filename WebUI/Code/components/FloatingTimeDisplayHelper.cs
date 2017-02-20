using System;

namespace Orchestrator.WebUI
{
	/// <summary>
	/// Provides a number of helper routines when displaying a floating time period in a UltraWebGrid control
	/// </summary>
	public class FloatingTimeDisplayHelper
	{
		public FloatingTimeDisplayHelper() {}

		#region Static Methods

		/// <summary>
		/// Returns the column ordinal that should contain the supplied DateTime value.
		/// </summary>
		/// <param name="datetime">The DateTime value to query for</param>
		///	<param name="startDateTime">The earliest DateTime that can be contained by the grid.</param>
		/// <param name="columnTimeSpan">The amount of time covered by an individual column</param>
		/// <param name="precedingOffset">The number of columns that appear before the time columns start</param>
		/// <param name="columnsDisplayed">The last column ordinal used to display DateTime values.</param>
		/// <returns>The column ordinal that should contain the supplied DateTime value</returns>
		public static int GetOrdinalOfApplicableTimeBlock(DateTime datetime, DateTime startDateTime, TimeSpan columnTimeSpan, int precedingOffset, int columnsDisplayed)
		{
			// Convert the DateTime to the start of it's relevant block
			datetime = NearestTimeBlockStart(datetime, columnTimeSpan);

			if (datetime <= startDateTime)
				return precedingOffset;

			// Get the difference between the DateTime and the and starting DateTime
			long ticks = (datetime.Subtract(startDateTime)).Ticks;

			// The ordinal to use is the result of dividing the tick offset by the number of ticks
			// represented by each column plus the number of preceding columns not used for display.			
			int cellOrdinal = (int) (ticks / columnTimeSpan.Ticks) + precedingOffset;
			
			// Return the ordinal to use, if this is greater than the maximum number, return the maximum
			if (cellOrdinal > columnsDisplayed)
				return columnsDisplayed;
			else
				return cellOrdinal;
		}

		/// <summary>
		/// Returns the DateTime that represents the start of the time block that the supplied DateTime
		/// should be display in.
		/// </summary>
		/// <param name="datetime">The DateTime to query for</param>
		/// <param name="columnTimeSpan">The amount of time covered by an individual column</param>
		/// <returns>The DateTime that represents the start of the containing time period</returns>
		public static DateTime NearestTimeBlockStart(DateTime datetime, TimeSpan columnTimeSpan)
		{
			// Get the modulus remainder number of ticks
			long ticks = datetime.Ticks % columnTimeSpan.Ticks;

			// Subtract the remainder from the supplied datetime
			return datetime.Subtract(new TimeSpan(ticks));
		}

		#endregion
	}
}
