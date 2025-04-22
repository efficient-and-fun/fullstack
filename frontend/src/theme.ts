import { createTheme } from "@mui/material/styles";

const theme = createTheme({
  components: {
    MuiCssBaseline: {
      styleOverrides: {
        body: {
          backgroundImage: 'linear-gradient(180deg, #49B759, #A7D558)',
          backgroundColor: 'white', // Fallback
          margin: 0,
        },
      },
    },
  },
  palette: {
    mode: "dark", // or 'light'
    background: {
      default: "#242424",
      paper: "transparent",
    },
    text: {
      primary: "rgba(255, 255, 255, 0.87)",
    },
  },
  typography: {
    fontFamily: "system-ui, Avenir, Helvetica, Arial, sans-serif",
  }
});

export default theme;