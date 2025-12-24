import { useEffect, useState } from 'react';

function ConcertsList() {
  const [concerts, setConcerts] = useState([]);

  useEffect(() => {
    fetch('http://localhost:8080/api/concerts') 
      .then(response => response.json())
      .then(data => setConcerts(data))
      .catch(error => console.error('Ошибка:', error));
  }, [])

  return (
    <div>
      <h1>Список концертов</h1>
      <ul>
        {concerts.map(concert => (
          <li key={concert.id}>
            <strong>{concert.name}</strong> — {concert.date}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default ConcertsList;