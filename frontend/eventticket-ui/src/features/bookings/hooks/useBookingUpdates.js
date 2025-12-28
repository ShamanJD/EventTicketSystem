import { useState, useEffect } from 'react';
import * as signalR from '@microsoft/signalr';

export const useBookingUpdates = (bookingId) => {
    const [status, setStatus] = useState(null);

    useEffect(() => {
        if (!bookingId) return;

        const connection = new signalR.HubConnectionBuilder()
            .withUrl('http://localhost:5002/hubs/booking')
            .withAutomaticReconnect()
            .build();

        connection.start()
            .then(() => {
                console.log('SignalR Connected');
                connection.invoke('JoinBookingGroup', bookingId)
                    .catch(err => console.error('Join group failed', err));
            })
            .catch(err => console.error('SignalR Connection Error: ', err));

        connection.on('StatusUpdated', (newStatus) => {
            console.log('Status updated:', newStatus);
            setStatus(newStatus);
        });

        return () => {
            connection.stop();
        };
    }, [bookingId]);

    return status;
};
