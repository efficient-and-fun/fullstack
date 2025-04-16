// Example: in your App.tsx or Calendar.tsx
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { StaticDatePicker } from "@mui/x-date-pickers/StaticDatePicker";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import dayjs, { Dayjs } from "dayjs";
import styles from "./Calendar.module.css";
import "dayjs/locale/de";

dayjs.locale("de");

interface CalendarProps {
  selectedDate: Dayjs;
  setSelectedDate: React.Dispatch<React.SetStateAction<dayjs.Dayjs>>;
}

const Calendar: React.FC<CalendarProps> = ({ selectedDate, setSelectedDate }) => {

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs} adapterLocale="de">
      <div className={styles.calendarContainer}>
        <StaticDatePicker
          displayStaticWrapperAs="desktop"
          value={selectedDate}
          onChange={(newValue) => setSelectedDate(newValue)}
          showDaysOutsideCurrentMonth
          sx={{
            ".MuiPickersCalendarHeader-label": {
              fontSize: "1.5rem",
              fontWeight: "bold",
              fontStyle: "italic",
              color: "#9DD258",
            },
            ".MuiPickersCalendarHeader-root": {
              width: "100%" /* Ensures the calendar header takes full width */,
            },
            ".MuiPickersDay-root.Mui-selected": {
              backgroundColor: "white",
              color: "black",
              "&:focus, &:hover": {      // Ensure hover and focus match selected style
                backgroundColor: "white",
                color: "black",
              },
            },
            ".MuiPickersDay-root": {
              color: "black", // changes text color of day numbers
            },
            ".MuiPickersDay-dayOutsideMonth": {
              color: "white",
              opacity: 0.5, // optional, default is lower
            },
            "& .MuiPickersYear-yearButton.Mui-selected": {
              backgroundColor: "white",
              color: "black",
              "&:focus, &:hover": {      // Ensure hover and focus match selected style
                backgroundColor: "white",
                color: "black",
              },
            },
            ".MuiDayCalendar-header": {
              borderBottom: "1px solid black", // ✅ Divider line under weekdays
              marginBottom: "8px", // Optional space after the line
            },
            ".MuiDayCalendar-weekDayLabel": {
    color: "black",
    fontWeight: "bold",
  },
          }}
        />
      </div>
    </LocalizationProvider>
  );
};

export default Calendar;
