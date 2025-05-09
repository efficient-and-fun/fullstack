import { useState } from 'react';
import styles from "./Form.module.css";
import { useNavigate } from 'react-router-dom';

const RegisterForm = () => {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password1, setPassword1] = useState('');
  const [password2, setPassword2] = useState('');
  const [isAGBAccepted, setAGBAccepted] = useState(true);
  const [errors, setErrors] = useState<string[]>([]); // Errors are stored here
  const navigate = useNavigate();
  const url = '/api/user/register';

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();

    const newErrors: string[] = []; // Temporary error list

    // Username validation
    if(!username) {
      newErrors.push('Plese enter a username.');
    }

    // Email validation
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
      newErrors.push('Please enter a valid email address.');
    }

    // Password validation
    if (password1 !== password2) {
      newErrors.push('Passwords do not match.');
    }

    if (password1.length < 8) {
      newErrors.push('Password must be at least 8 characters long.');
    }

    if (!/[A-Z]/.test(password1)) {
      newErrors.push('Password must contain at least one uppercase letter.');
    }

    if (!/[a-z]/.test(password1)) {
      newErrors.push('Password must contain at least one lowercase letter.');
    }

    if (!/[0-9]/.test(password1)) {
      newErrors.push('Password must contain at least one number.');
    }

    if (!/[!@#$%^&*]/.test(password1)) {
      newErrors.push('Password must contain at least one special character (!@#$%^&*).');
    }

    if( !isAGBAccepted) {
      newErrors.push('Please accept the terms and conditions.');
    }

    // If there are errors, set them in the state and stop execution
    if (newErrors.length > 0) {
      setErrors(newErrors);
      return;
    } else {
        setErrors([]); // Clear errors if no validation issues
    }

    // Try to make the API call
    try {
        const res = await fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, email, password: password1, password2: password2, profilePicturePath: "C:/temp/photo", isAGBAccepted: true }),
        });
        const data = await res.json();

        if (res.ok) {
        localStorage.setItem("authToken", data.token);
        navigate("/");
        } else {
        setErrors([data.message || 'Registration failed. Please try again.']);
        }
    } catch (error) {
        // Catch network or other unexpected errors
        setErrors(['An unexpected error occurred. Please try again later.']);
        console.error('Error during registration:', error);
    }
  };

  return (
    <form onSubmit={handleRegister} className={styles.container}>
      <h1 className={styles.title}>Registration</h1>

      <input
        className={`${styles.inputField} cy-register-username`}
        placeholder="Username"
        type="username"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        required
      />
      <input
        className={`${styles.inputField} cy-register-email`}
        placeholder="Email"
        type="email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        required
      />
      <input
        className={`${styles.inputField} cy-register-pwd1`}
        placeholder="Password"
        type="password"
        value={password1}
        onChange={(e) => setPassword1(e.target.value)}
        required
      />
      <input
        className={`${styles.inputField} cy-register-pwd2`}
        placeholder="Confirm Password"
        type="password"
        value={password2}
        onChange={(e) => setPassword2(e.target.value)}
        required
      />

      <div className={`${styles.checkboxContainer} cy-register-checkboy`}>
        <input
          type="checkbox"
          id="agb"
          checked={isAGBAccepted}
          onChange={(e) => setAGBAccepted(e.target.checked)}
        />
        <label htmlFor="agb">I accept the terms and conditions</label>
      </div>
      <button className={`${styles.btn} cy-register-submitbutton`} type="submit">
        Sign up
      </button>

      {/* Error display */}
      {errors.length > 0 && (
        <div className={styles.errorContainer}>
          {errors.map((error, index) => (
            <p key={index} className={`${styles.errorText} cy-register-erros`}>
              {error}
            </p>
          ))}
        </div>
      )}
    </form>
  );
};

export default RegisterForm;