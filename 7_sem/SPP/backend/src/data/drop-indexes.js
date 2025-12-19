import mongoose from 'mongoose';

const MONGODB_URI = process.env.MONGODB_URI || 'mongodb://localhost:27017/spp_db';

const dropIndexes = async () => {
    try {
        await mongoose.connect(MONGODB_URI);
        console.log('Connected to MongoDB');

        const db = mongoose.connection.db;
        const usersCollection = db.collection('users');

        try {
            await usersCollection.dropIndex('email_1');
            console.log('Dropped email_1 index');
        } catch (error) {
            console.log('email_1 index does not exist or already dropped');
        }

        console.log('Indexes cleaned successfully');
        process.exit(0);
    } catch (error) {
        console.error('Error cleaning indexes:', error);
        process.exit(1);
    }
};

dropIndexes();
