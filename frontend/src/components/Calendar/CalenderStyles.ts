import { SxProps, Theme } from "@mui/material/styles";

export const calendarStyles: SxProps<Theme> = {
  ".MuiPickersCalendarHeader-label": {
    fontSize: "1.5rem",
    fontWeight: "bold",
    fontStyle: "italic",
    color: "#9DD258",
  },
  ".MuiPickersCalendarHeader-root": {
    width: "100%", // Ensures the calendar header takes full width
  },
  ".MuiPickersDay-root.Mui-selected": {
    backgroundColor: "white",
    color: "black",
    "&:focus, &:hover": {
      backgroundColor: "white",
      color: "black",
    },
  },
  ".MuiPickersDay-root": {
    color: "black", // Changes text color of day numbers
  },
  ".MuiPickersDay-dayOutsideMonth": {
    color: "white",
    opacity: 0.5, // Optional, default is lower
  },
  "& .MuiPickersYear-yearButton.Mui-selected": {
    backgroundColor: "white",
    color: "black",
    "&:focus, &:hover": {
      backgroundColor: "white",
      color: "black",
    },
  },
  ".MuiDayCalendar-header": {
    borderBottom: "1px solid black", // Divider line under weekdays
    marginBottom: "8px", // Optional space after the line
  },
  ".MuiDayCalendar-weekDayLabel": {
    color: "black",
    fontWeight: "bold",
  },
};