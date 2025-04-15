// Example: in your App.tsx or Calendar.tsx
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { StaticDatePicker } from "@mui/x-date-pickers/StaticDatePicker";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import dayjs, { Dayjs } from "dayjs";
import { useState } from "react";
import styles from "./Calendar.module.css";

const Calendar = () => {
  const [value, setValue] = useState<Dayjs | null>(dayjs());

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <div className={styles.calendarContainer}>
        <StaticDatePicker
          displayStaticWrapperAs="desktop"
          value={value}
          onChange={(newValue) => setValue(newValue)}
          sx={{
            ".MuiPickersCalendarHeader-label": {
              fontSize: "1.5rem",
              fontWeight: "bold",
              fontStyle: "italic",
              color: "lightgreen",
            },
            ".MuiPickersCalendarHeader-root": {
              width: "100%" /* Ensures the calendar header takes full width */,
            },
          }}
        />
      </div>
    </LocalizationProvider>
  );
};

export default Calendar;
