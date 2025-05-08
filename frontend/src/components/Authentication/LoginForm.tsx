import { useState } from "react";
import styles from "./Form.module.css";

const LoginForm = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [errors, setErrors] = useState<string[]>([]); // Errors are stored here
  var url = "/api/user/login";

  const handleLogin = async (e) => {
    e.preventDefault();

    const newErrors: string[] = [];
    // Validierung: Überprüfen, ob Felder leer sind
    if (!email.trim()) {
      newErrors.push("Email cannot be empty.");
    }
    if (!password.trim()) {
      newErrors.push("Password cannot be empty.");
    }

    // Wenn Fehler vorliegen, setze sie in den State und breche ab
    if (newErrors.length > 0) {
      setErrors(newErrors);
      return;
    } else {
      setErrors([]); // Fehler zurücksetzen, wenn keine Validierungsprobleme vorliegen
    }

    // API-Aufruf in einem try-catch-Block
    try {
      const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
      });

      const data = await res.json();

      if (res.ok) {
        localStorage.setItem("authToken", data.token);
        window.location.href = '/';
      } else {
        // Fehler vom Server anzeigen
        setErrors([data.message || "Login failed. Please try again."]);
      }
    } catch (error) {
      // Netzwerk- oder andere unerwartete Fehler abfangen
      setErrors(["An unexpected error occurred. Please try again later."]);
      console.error("Error during login:", error);
    }
  };

  return (
    <form onSubmit={handleLogin} className={styles.container}>
      <h1 className={styles.title}>Login</h1>
      <input
        className={styles.inputField}
        placeholder="Email"
        type="email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        required
      />
      <input
        className={styles.inputField}
        placeholder="Password"
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        required
      />
      <button className={styles.btn} type="submit">
        Sign in
      </button>

      {/* Error display */}
      {errors.length > 0 && (
        <div className={styles.errorContainer}>
          {errors.map((error, index) => (
            <p key={index} className={styles.errorText}>
              {error}
            </p>
          ))}
        </div>
      )}
    </form>
  );
};

export default LoginForm;
