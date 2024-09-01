import type { NextApiRequest, NextApiResponse } from 'next';
import axios from 'axios';

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  if (req.method === 'POST') {
    const { email } = req.body;

    try {
      const response = await axios.post('http://localhost:5087/api/User/renew-subscription', JSON.stringify(email), {
        headers: {
          'Content-Type': 'application/json',
          'accept': '*/*',
        },
        withCredentials: true,
      });

      const paymentLink = response.data; // Предполагается, что ссылка для оплаты находится в response.data
      res.status(200).json({ paymentLink });

    } catch (error) {
      console.error('Error renewing subscription:', error);
      res.status(500).json({ message: 'Ошибка при продлении подписки' });
    }
  } else {
    res.setHeader('Allow', ['POST']);
    res.status(405).end(`Method ${req.method} Not Allowed`);
  }
}
