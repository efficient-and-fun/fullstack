import { useState } from 'react';
import styles from "./Form.module.css";

const RegisterForm = () => {
  const [email, setEmail] = useState('');
  const [password1, setPassword1] = useState('');
  const [password2, setPassword2] = useState('');
  var url = 'http://localhost:5000/api/register';

  const handleRegister = async (e) => {
    e.preventDefault();
    const res = await fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password1 }),
    });
    if (res.ok) {
      alert('Registrierung erfolgreich!');
      window.location.href = '/login';
    } else {
      alert('Registrierung fehlgeschlagen');
    }
  };

  return (
    <form onSubmit={handleRegister} className={styles.container}>
        <h1 className={styles.title}>Registration</h1>
        <input className={styles.inputField} placeholder='Email' type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
        <input className={styles.inputField} placeholder='Password' type="password" value={password1} onChange={(e) => setPassword1(e.target.value)} required />
        <input className={styles.inputField} placeholder='Password' type="password" value={password2} onChange={(e) => setPassword2(e.target.value)} required />
        <button className={styles.btn} type="submit">Sign up</button>
    </form>
  );
};

export default RegisterForm;
