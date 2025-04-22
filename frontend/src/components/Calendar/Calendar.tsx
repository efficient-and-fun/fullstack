// Example: in your App.tsx or Calendar.tsx
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { StaticDatePicker } from "@mui/x-date-pickers/StaticDatePicker";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import dayjs, { Dayjs } from "dayjs";
import styles from "./Calendar.module.css";
import "dayjs/locale/de";
import { calendarStyles } from "./CalenderStyles";

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
          sx={calendarStyles}
        />
      </div>
    </LocalizationProvider>
  );
};

export default Calendar;
