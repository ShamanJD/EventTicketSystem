import { useEffect, useState } from 'react';

function ConcertsList() {
  const [concerts, setConcerts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetch('https://localhost:7000/api/concerts')
      .then(response => response.json())
      .then(data => {
        setConcerts(data);
        setLoading(false);
      })
      .catch(error => {
        console.error('Ошибка:', error);
        setError('Не удалось загрузить концерты');
        setLoading(false);
      });
  }, []);

  if (loading) {
    return (
      <div style={{ padding: '2rem' }}>
        <h2 style={{ marginBottom: '2rem' }}>Концерты</h2>
        <div style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fill, minmax(320px, 1fr))',
          gap: '1.5rem'
        }}>
          {[1, 2, 3, 4].map(i => (
            <div key={i} className="card animate-pulse" style={{ height: '280px' }}>
              <div style={{
                width: '100%',
                height: '160px',
                background: 'var(--bg-card-hover)',
                borderRadius: '12px',
                marginBottom: '1rem'
              }} />
              <div style={{
                width: '70%',
                height: '20px',
                background: 'var(--bg-card-hover)',
                borderRadius: '8px',
                marginBottom: '0.5rem'
              }} />
              <div style={{
                width: '50%',
                height: '16px',
                background: 'var(--bg-card-hover)',
                borderRadius: '8px'
              }} />
            </div>
          ))}
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{
        padding: '2rem',
        textAlign: 'center'
      }}>
        <div className="animate-fade-in" style={{
          maxWidth: '400px',
          margin: '4rem auto',
          padding: '2rem',
          background: 'rgba(245, 87, 108, 0.1)',
          border: '1px solid rgba(245, 87, 108, 0.3)',
          borderRadius: '16px'
        }}>
          <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⚠️</div>
          <h3 style={{ marginBottom: '0.5rem' }}>Ошибка загрузки</h3>
          <p style={{ color: 'var(--text-secondary)' }}>{error}</p>
        </div>
      </div>
    );
  }

  return (
    <div style={{ padding: '2rem' }}>
      <div style={{
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginBottom: '2rem'
      }}>
        <h2 className="gradient-text">Предстоящие концерты</h2>
        <div style={{
          padding: '0.5rem 1rem',
          background: 'var(--bg-card)',
          borderRadius: '12px',
          fontSize: '0.875rem',
          fontWeight: '600',
          color: 'var(--text-secondary)'
        }}>
          {concerts.length} {concerts.length === 1 ? 'концерт' : 'концертов'}
        </div>
      </div>

      {concerts.length === 0 ? (
        <div className="animate-fade-in" style={{
          textAlign: 'center',
          padding: '4rem 2rem'
        }}>
          <div style={{ fontSize: '4rem', marginBottom: '1rem' }}>🎵</div>
          <h3 style={{ marginBottom: '0.5rem' }}>Пока нет концертов</h3>
          <p style={{ color: 'var(--text-secondary)' }}>
            Скоро здесь появятся новые мероприятия
          </p>
        </div>
      ) : (
        <div style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fill, minmax(320px, 1fr))',
          gap: '1.5rem'
        }}>
          {concerts.map((concert, index) => (
            <div
              key={concert.id}
              className="card glass-hover animate-fade-in"
              style={{
                animationDelay: `${index * 0.1}s`,
                cursor: 'pointer',
                overflow: 'hidden'
              }}
            >
              <div style={{
                width: '100%',
                height: '160px',
                background: `linear-gradient(135deg, 
                  hsl(${(index * 60) % 360}, 70%, 60%) 0%, 
                  hsl(${(index * 60 + 60) % 360}, 70%, 50%) 100%)`,
                borderRadius: '12px',
                marginBottom: '1rem',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                fontSize: '3rem',
                position: 'relative',
                overflow: 'hidden'
              }}>
                <div style={{
                  position: 'absolute',
                  inset: 0,
                  background: 'rgba(0, 0, 0, 0.2)',
                  backdropFilter: 'blur(2px)'
                }} />
                <span style={{ position: 'relative', zIndex: 1 }}>🎸</span>
              </div>

              <div>
                <h3 style={{
                  fontSize: '1.25rem',
                  fontWeight: '700',
                  marginBottom: '0.5rem',
                  overflow: 'hidden',
                  textOverflow: 'ellipsis',
                  whiteSpace: 'nowrap'
                }}>
                  {concert.name}
                </h3>

                <div style={{
                  display: 'flex',
                  alignItems: 'center',
                  gap: '0.5rem',
                  color: 'var(--text-secondary)',
                  fontSize: '0.875rem',
                  marginBottom: '1rem'
                }}>
                  <span>📅</span>
                  <span>{new Date(concert.date).toLocaleDateString('ru-RU', {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric'
                  })}</span>
                </div>

                <button style={{
                  width: '100%',
                  background: 'var(--primary-gradient)',
                  color: 'white',
                  padding: '0.75rem',
                  borderRadius: '10px',
                  fontWeight: '600',
                  fontSize: '0.875rem',
                  transition: 'all 0.3s ease'
                }}
                  onMouseEnter={(e) => {
                    e.target.style.transform = 'translateY(-2px)';
                    e.target.style.boxShadow = '0 8px 16px rgba(102, 126, 234, 0.4)';
                  }}
                  onMouseLeave={(e) => {
                    e.target.style.transform = 'translateY(0)';
                    e.target.style.boxShadow = 'none';
                  }}
                >
                  Купить билет
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default ConcertsList;
