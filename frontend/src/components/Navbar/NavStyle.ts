import { SxProps, Theme } from "@mui/material/styles";

export const bottomNavigationStyles: SxProps<Theme> = {
  borderTopLeftRadius: 16,
  borderTopRightRadius: 16,
  position: "fixed",
  bottom: 0,
  left: 0,
  right: 0,
  bgcolor: "#202020",
};

export const bottomNavigationActionStyles: SxProps<Theme> = {
  outline: "none",
  "&:focus": {
    outline: "none",
  },
  color: "white",
  "&.Mui-selected": {
    color: "#49B759",
    "& .MuiBottomNavigationAction-label": {
      color: "#49B759",
    },
  },
};