#include "../inc/JobQueue.hpp"

void JobQueue::enqueue(const std::function<void()>& job) {
    std::lock_guard<std::mutex> lock(_mtx);
    _jobs.push(job);
}

std::optional<std::function<void()>> JobQueue::dequeue() {
    std::lock_guard<std::mutex> lock(_mtx);
    if (!_jobs.empty()) {
        auto job = _jobs.front();
        _jobs.pop();
        return job;
    }
    return std::nullopt;
}

bool JobQueue::empty() {
    std::lock_guard<std::mutex> lock(_mtx);
    return _jobs.empty();
}
