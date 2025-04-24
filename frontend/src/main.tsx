import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import theme from "./theme";
import App from "./App";
import { ThemeProvider } from "@mui/material/styles";
import CssBaseline from "@mui/material/CssBaseline";

function mountApp() {
  createRoot(document.getElementById("root")!).render(
    <StrictMode>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <App />
      </ThemeProvider>
    </StrictMode>
  );
}

if (import.meta.env.MODE == "Mock") {
  import('./mocks/browser').then(({ worker }) => {
    worker
      .start({ serviceWorker: { url: `${import.meta.env.BASE_URL}mockServiceWorker.js`,
      },
      onUnhandledRequest: "bypass",
     })
      .then(() => {
        mountApp();
      });
  });
} else {
  mountApp();
}
