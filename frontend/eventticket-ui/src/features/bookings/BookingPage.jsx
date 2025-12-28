import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { createBooking } from './api/bookingsApi';
import { useBookingUpdates } from './hooks/useBookingUpdates';

const BookingPage = () => {
    const [bookingId, setBookingId] = useState(null);
    const concertId = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
    const amount = 100;

    const mutation = useMutation({
        mutationFn: () => createBooking(concertId, amount),
        onSuccess: (data) => {
            console.log("Booking created:", data);
            setBookingId(data.id || data.bookingId);
        }
    });

    const status = useBookingUpdates(bookingId);

    const getStatusConfig = (s) => {
        const configs = {
            'Confirmed': {
                color: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
                icon: '✓',
                text: 'Подтверждено'
            },
            'Cancelled': {
                color: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
                icon: '✗',
                text: 'Отменено'
            },
            'Processing': {
                color: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)',
                icon: '⏳',
                text: 'Обработка'
            }
        };
        return configs[s] || configs['Processing'];
    };

    return (
        <div style={{
            maxWidth: '800px',
            margin: '0 auto',
            padding: '2rem'
        }}>
            <div className="animate-fade-in">
                <h2 className="gradient-text" style={{ marginBottom: '1rem' }}>
                    Бронирование билета
                </h2>
                <p style={{
                    color: 'var(--text-secondary)',
                    marginBottom: '2rem',
                    fontSize: '1.05rem'
                }}>
                    Демонстрация системы бронирования с real-time обновлениями
                </p>

                <div className="glass" style={{
                    padding: '2rem',
                    borderRadius: '20px',
                    marginBottom: '1.5rem'
                }}>
                    <div style={{
                        display: 'grid',
                        gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
                        gap: '1.5rem',
                        marginBottom: '2rem'
                    }}>
                        <div>
                            <div style={{
                                fontSize: '0.875rem',
                                color: 'var(--text-muted)',
                                marginBottom: '0.5rem',
                                fontWeight: '600'
                            }}>
                                ID Концерта
                            </div>
                            <div style={{
                                fontSize: '0.875rem',
                                fontFamily: 'monospace',
                                color: 'var(--text-secondary)',
                                background: 'var(--bg-card-hover)',
                                padding: '0.5rem',
                                borderRadius: '8px'
                            }}>
                                {concertId.substring(0, 8)}...
                            </div>
                        </div>
                        <div>
                            <div style={{
                                fontSize: '0.875rem',
                                color: 'var(--text-muted)',
                                marginBottom: '0.5rem',
                                fontWeight: '600'
                            }}>
                                Стоимость
                            </div>
                            <div style={{
                                fontSize: '1.5rem',
                                fontWeight: '700',
                                background: 'var(--primary-gradient)',
                                WebkitBackgroundClip: 'text',
                                WebkitTextFillColor: 'transparent',
                                backgroundClip: 'text'
                            }}>
                                {amount} ₽
                            </div>
                        </div>
                    </div>

                    <button
                        onClick={() => mutation.mutate()}
                        disabled={mutation.isPending || bookingId}
                        style={{
                            width: '100%',
                            background: bookingId
                                ? 'var(--bg-card-hover)'
                                : 'var(--primary-gradient)',
                            color: 'white',
                            padding: '1rem 2rem',
                            borderRadius: '12px',
                            fontSize: '1.05rem',
                            fontWeight: '600',
                            cursor: (mutation.isPending || bookingId) ? 'not-allowed' : 'pointer',
                            transition: 'all 0.3s ease',
                            boxShadow: bookingId
                                ? 'none'
                                : '0 4px 16px rgba(102, 126, 234, 0.4)',
                        }}
                        onMouseEnter={(e) => {
                            if (!mutation.isPending && !bookingId) {
                                e.target.style.transform = 'translateY(-2px)';
                                e.target.style.boxShadow = '0 8px 24px rgba(102, 126, 234, 0.5)';
                            }
                        }}
                        onMouseLeave={(e) => {
                            if (!mutation.isPending && !bookingId) {
                                e.target.style.transform = 'translateY(0)';
                                e.target.style.boxShadow = '0 4px 16px rgba(102, 126, 234, 0.4)';
                            }
                        }}
                    >
                        {mutation.isPending ? (
                            <span style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '0.5rem' }}>
                                <span className="animate-pulse">⏳</span> Создание брони...
                            </span>
                        ) : bookingId ? (
                            <span style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '0.5rem' }}>
                                ✓ Бронь создана
                            </span>
                        ) : (
                            'Забронировать билет'
                        )}
                    </button>
                </div>

                {mutation.isError && (
                    <div className="animate-fade-in glass" style={{
                        padding: '1.25rem',
                        background: 'rgba(245, 87, 108, 0.1)',
                        border: '1px solid rgba(245, 87, 108, 0.3)',
                        borderRadius: '16px',
                        marginBottom: '1.5rem'
                    }}>
                        <div style={{
                            display: 'flex',
                            alignItems: 'center',
                            gap: '0.75rem',
                            color: '#f5576c'
                        }}>
                            <span style={{ fontSize: '1.5rem' }}>⚠️</span>
                            <div>
                                <div style={{ fontWeight: '600', marginBottom: '0.25rem' }}>
                                    Ошибка бронирования
                                </div>
                                <div style={{ fontSize: '0.875rem', opacity: 0.9 }}>
                                    {mutation.error.message}
                                </div>
                            </div>
                        </div>
                    </div>
                )}

                {bookingId && (
                    <div className="animate-fade-in glass" style={{
                        padding: '1.5rem',
                        borderRadius: '16px'
                    }}>
                        <div style={{
                            display: 'flex',
                            justifyContent: 'space-between',
                            alignItems: 'center',
                            marginBottom: '1rem'
                        }}>
                            <div>
                                <div style={{
                                    fontSize: '0.875rem',
                                    color: 'var(--text-muted)',
                                    marginBottom: '0.25rem',
                                    fontWeight: '600'
                                }}>
                                    ID Брони
                                </div>
                                <div style={{
                                    fontFamily: 'monospace',
                                    fontSize: '0.95rem',
                                    color: 'var(--text-secondary)'
                                }}>
                                    {bookingId}
                                </div>
                            </div>
                        </div>

                        <div style={{
                            marginTop: '1.5rem',
                            paddingTop: '1.5rem',
                            borderTop: '1px solid var(--border-color)'
                        }}>
                            <div style={{
                                fontSize: '0.875rem',
                                color: 'var(--text-muted)',
                                marginBottom: '0.75rem',
                                fontWeight: '600'
                            }}>
                                Статус бронирования
                            </div>
                            <div style={{
                                display: 'inline-flex',
                                alignItems: 'center',
                                gap: '0.5rem',
                                padding: '0.75rem 1.25rem',
                                background: getStatusConfig(status || 'Processing').color,
                                borderRadius: '12px',
                                color: 'white',
                                fontWeight: '600',
                                fontSize: '0.95rem',
                                boxShadow: '0 4px 12px rgba(0, 0, 0, 0.2)'
                            }}>
                                <span style={{ fontSize: '1.25rem' }}>
                                    {getStatusConfig(status || 'Processing').icon}
                                </span>
                                <span>
                                    {status ? getStatusConfig(status).text : 'Ожидание оплаты...'}
                                </span>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
};

export default BookingPage;

