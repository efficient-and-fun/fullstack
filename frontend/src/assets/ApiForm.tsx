import { useState } from "react";

const ApiForm = () => {
  const [inputValue, setInputValue] = useState("");
  const [response, setResponse] = useState<string | null>(null);
  
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const res = await fetch('/api/first/echo', {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ message: inputValue }),
      });

      const data = await res.text();
      setResponse(data);
      console.log(data) // Store only the title value
    } catch (error) {
      console.error("API Fehler:", error);
      setResponse("Fehler beim Abrufen der Daten");
    }
  };

  return (
    <div>
      <h2>API Test</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          value={inputValue}
          onChange={(e) => setInputValue(e.target.value)}
          placeholder="Gib etwas ein..."
        />
        <button type="submit">Senden</button>
      </form>
      <pre>{response || "Noch keine Antwort"}</pre>
    </div>
  );
};

export default ApiForm;
