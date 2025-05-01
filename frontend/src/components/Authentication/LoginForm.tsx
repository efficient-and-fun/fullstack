import { useState } from 'react';
import styles from "./Form.module.css";

const LoginForm = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  var url = 'http://localhost:5000/api/login';

  const handleLogin = async (e) => {
    e.preventDefault();
    const res = await fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password }),
    });
    const data = await res.json();
    if (res.ok) {
      localStorage.setItem('token', data.token);
      window.location.href = '/dashboard';
    } else {
      alert('Login fehlgeschlagen');
    }
  };

  return (
    <form onSubmit={handleLogin} className={styles.container}> 
        <h1 className={styles.title}>Login</h1>
        <input className={styles.inputField} placeholder="Email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
        <input className={styles.inputField} placeholder="Password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
        <button className={styles.btn} type="submit">Sign in</button>
    </form>
  );
};

export default LoginForm;
