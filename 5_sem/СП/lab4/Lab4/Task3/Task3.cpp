// ReSharper disable CppClangTidyBugproneNarrowingConversions
// ReSharper disable CppClangTidyClangDiagnosticShorten64To32
#include <fstream>
#include <vector>
#include <string>
#include <algorithm>
#include <thread>
#include <mutex>
#include <optional>
#include <functional>
#include <chrono>
#include <iostream>

#include "inc/JobQueue.hpp"

const int INPUT_THREAD = 4;
const int NUM_SORT_THREADS = 4;
const int NUM_JOBS = NUM_SORT_THREADS * 2;

JobQueue jobQueue;
std::atomic<bool> isProductionContinuing{true};
std::vector<std::vector<std::string>> sortedParts;
std::vector<std::string> input;
std::mutex coutMtx;

std::vector<std::string> readFile(const std::string& filename) {
    std::vector<std::string> result;
    
    std::ifstream file(filename);
    std::string line;
    while (std::getline(file, line)) {
        result.push_back(line);
    }

    return result;
}

void inputThread(const int s, const int e) {
    const auto start = std::chrono::steady_clock::now();
    
    const int partSize = input.size() / NUM_JOBS;
    for (int i = s; i < e; ++i) {
        const int start = i * partSize;
        const int end = i == NUM_JOBS - 1 ? input.size() : start + partSize;
        std::vector part(input.begin() + start, input.begin() + end);

        jobQueue.enqueue([part = std::move(part), i]() mutable {
            std::ranges::sort(part);
            sortedParts[i] = std::move(part);
        });
    }

    const auto end = std::chrono::steady_clock::now();
    const auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();
    {
        std::lock_guard<std::mutex> lock(coutMtx);
        std::cout << "Input thread " << std::this_thread::get_id() << " ran for " << duration << " ms.\n";
    }
}

std::vector<std::thread> createInputThreads(const std::string& filename) {
    std::vector<std::thread> inputThreads;
    inputThreads.reserve(INPUT_THREAD);
    
    input = readFile(filename);
    constexpr int partSize = NUM_JOBS / INPUT_THREAD;
    for (int i = 0; i < INPUT_THREAD; i++) {
        const int start = i * partSize;
        const int end = i == INPUT_THREAD - 1 ? NUM_JOBS : start + partSize;
        inputThreads.emplace_back([start, end] {
            inputThread(start, end);
        });
    }
    
    return inputThreads;
}

void sortWorker() {
    while (isProductionContinuing || !jobQueue.empty()) {
        auto jobOpt = jobQueue.dequeue();
        if (jobOpt) {
            const auto start = std::chrono::steady_clock::now();
            (*jobOpt)();
            const auto end = std::chrono::steady_clock::now();
            const auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();
            {
                std::lock_guard<std::mutex> lock(coutMtx);
                std::cout << "Sort thread " << std::this_thread::get_id() << " ran for " << duration << " ms.\n";
            }
        } else {
            std::this_thread::sleep_for(std::chrono::milliseconds(1));
        }
    }
}

std::vector<std::string> mergeSortedParts() {
    std::vector<std::string> result;
    std::vector<int> indices(sortedParts.size(), 0);

    auto compare = [&](const int i, const int j) {
        return sortedParts[i][indices[i]] > sortedParts[j][indices[j]];
    };

    while (true) {
        int minIndex = -1;
        for (size_t i = 0; i < sortedParts.size(); ++i) {
            if (indices[i] < sortedParts[i].size() && (minIndex == -1 || compare(minIndex, i))) {
                minIndex = i;
            }
        }
        if (minIndex == -1)
            break;

        result.push_back(sortedParts[minIndex][indices[minIndex]]);
        indices[minIndex]++;
    }

    return result;
}

void outputThread(const std::string& outputFilename) {
    const auto start = std::chrono::steady_clock::now();
    const auto merged = mergeSortedParts();
    const auto end = std::chrono::steady_clock::now();
    const auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();
    {
        std::lock_guard<std::mutex> lock(coutMtx);
        std::cout << "Output thread merged for " << duration << " ms.\n";
    }
    
    std::ofstream outFile(outputFilename);
    for (const auto& line : merged) {
        outFile << line << "\n";
    }
}

void run() {
    std::vector<std::thread> inputThreads = createInputThreads("input.txt");
    
    std::vector<std::thread> sorts;
    sorts.reserve(NUM_SORT_THREADS);
    for (int i = 0; i < NUM_SORT_THREADS; ++i) {
        sorts.emplace_back(sortWorker);
    }

    for (auto& t : inputThreads) {
        t.join();
    }
    isProductionContinuing = false;
    for (auto& t : sorts) {
        t.join();
    }

    outputThread("output.txt");
}

int main() {
    sortedParts.resize(NUM_JOBS);
    const auto start = std::chrono::steady_clock::now();
    run();
    const auto end = std::chrono::steady_clock::now();
    const auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();
    std::cout << "\nThe program ran for " << duration << " ms.\n";
    return 0;
}
