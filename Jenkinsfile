pipeline {
    agent any

    stages {

        stage('Checkout') {
            steps {
                // Jenkins automatically checks out the repo — this stage just makes it visible in the UI
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                // Restore all NuGet packages
                sh 'dotnet restore RecLeagueAnalytics.sln'
            }
        }

        stage('Build') {
            steps {
                // Compile the entire solution in Release mode
                sh 'dotnet build RecLeagueAnalytics.sln -c Release --no-restore'
            }
        }

        stage('Test') {
            steps {
                // Run all unit tests
                sh 'dotnet test RecLeagueAnalytics.sln --no-build -c Release'
            }
        }

        stage('Docker Build') {
            steps {
                // Build the API Docker image and tag it
                sh 'docker build -t recleague-api:latest -f src/RecLeague.API/Dockerfile .'
            }
        }

    }

    post {
        success {
            echo 'Pipeline passed — build, tests, and Docker image all succeeded.'
        }
        failure {
            echo 'Pipeline failed — check the stage logs above for details.'
        }
    }
}
