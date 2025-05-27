#include <iostream>

#include "inc/JobQueue.hpp"

constexpr int NUM_PRODUCERS = 3;
constexpr int JOBS_PER_PRODUCER = 5;
constexpr int NUM_CONSUMERS = 2;

JobQueue jobQueue;
std::mutex coutMtx;
std::atomic<bool> isProductionContinuing{true};

void startProducers(std::vector<std::thread>& producers);
void startConsumers(std::vector<std::thread>& consumers);
void waitForThreads(std::vector<std::thread>& threads);

int main() {
    std::vector<std::thread> producers;
    startProducers(producers);
    std::vector<std::thread> consumers;
    startConsumers(consumers);

    waitForThreads(producers);
    isProductionContinuing = false;
    waitForThreads(consumers);
    
    return 0;
}

void startProducers(std::vector<std::thread>& producers) {
    auto producer = [&](const int id) {
        for (int i = 0; i < JOBS_PER_PRODUCER; ++i) {
            jobQueue.enqueue([id, i]() {
                {
                    std::lock_guard<std::mutex> lock(coutMtx);
                    std::cout << "Producer " << id << " created task " << i << "\n";
                }
                std::this_thread::sleep_for(std::chrono::milliseconds(100));
            });
        }
    };
    
    producers.reserve(NUM_PRODUCERS);
    for (int i = 0; i < NUM_PRODUCERS; ++i) {
        producers.emplace_back(producer, i + 1);
    }
}

void startConsumers(std::vector<std::thread>& consumers) {
    auto consumer = [&](const int id) {
        while (isProductionContinuing || !jobQueue.empty()) {
            auto jobOpt = jobQueue.dequeue();
            if (jobOpt) {
                (*jobOpt)();
                std::lock_guard<std::mutex> lock(coutMtx);
                std::cout << "Consumer " << id << " completed task\n";
            } else {
                std::this_thread::sleep_for(std::chrono::milliseconds(100));
            }
        }
    };

    consumers.reserve(NUM_CONSUMERS);
    for (int i = 0; i < NUM_CONSUMERS; ++i) {
        consumers.emplace_back(consumer, i + 1);
    }
}

void waitForThreads(std::vector<std::thread>& threads) {
    for (auto& t : threads) {
        t.join();
    }
}