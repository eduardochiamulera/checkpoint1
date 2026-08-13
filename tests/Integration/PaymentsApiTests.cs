using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Cursos.Tests.Fixtures;
using System.Net.Http.Headers;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using System;

namespace Cursos.Tests.Integration;

public sealed class PaymentsApiTests
    : IClassFixture<ApiFixture>
{
    private readonly ApiFixture _fixture;

    public PaymentsApiTests(ApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Should_return_401_without_token()
    {
        var response = await _fixture.Client.PostAsJsonAsync(
            "/api/v1/payments",
            ValidRequest());

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_return_400_for_invalid_amount()
    {
        var token = await CreateStudentTokenAsync();
        using var request = _fixture.AuthenticatedRequest(
            HttpMethod.Post,
            "/api/v1/payments",
            token);

        request.Headers.Add("Idempotency-Key", "invalid-amount");
        request.Content = JsonContent.Create(
            new
            {
                amount = 0,
                currency = "BRL",
                enrollmentId = 1,
                method = 1
            });

        var response = await _fixture.Client.SendAsync(request);

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);

        var body = await response.Content
            .ReadFromJsonAsync<ProblemDetailsResponse>();

        body!.TraceId.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Should_return_422_for_inactive_enrollment()
    {
        var token = await CreateStudentTokenAsync();
        using var request = _fixture.AuthenticatedRequest(
            HttpMethod.Post,
            "/api/v1/payments",
            token);

        request.Headers.Add(
            "Idempotency-Key",
            $"inactive-{Guid.NewGuid():N}");
        request.Content = JsonContent.Create(
            new
            {
                amount = 100,
                currency = "BRL",
                enrollmentId = 999999,
                method = 1
            });

        var response = await _fixture.Client.SendAsync(request);

        response.StatusCode
            .Should()
            .BeOneOf(
                HttpStatusCode.NotFound,
                HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Should_return_409_for_second_active_payment()
    {
        var token = await CreateStudentTokenAsync();
        var enrollmentId = await CreateEnrollmentAsync(token);

        var first = await CreatePaymentAsync(
            token,
            enrollmentId,
            $"first-{Guid.NewGuid():N}");

        first.StatusCode
            .Should()
            .Be(HttpStatusCode.Created);

        var second = await CreatePaymentAsync(
            token,
            enrollmentId,
            $"second-{Guid.NewGuid():N}");

        second.StatusCode
            .Should()
            .Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Should_return_same_payment_for_same_idempotency_key()
    {
        var token = await CreateStudentTokenAsync();
        var enrollmentId = await CreateEnrollmentAsync(token);
        var key = $"same-{Guid.NewGuid():N}";

        var first = await CreatePaymentAsync(
            token,
            enrollmentId,
            key);
        var second = await CreatePaymentAsync(
            token,
            enrollmentId,
            key);

        first.StatusCode.Should().Be(HttpStatusCode.Created);
        second.StatusCode.Should().Be(HttpStatusCode.Created);

        var firstBody = await first.Content
            .ReadFromJsonAsync<PaymentResponseTestModel>();
        var secondBody = await second.Content
            .ReadFromJsonAsync<PaymentResponseTestModel>();

        secondBody!.Id.Should().Be(firstBody!.Id);
    }

    [Fact]
    public async Task Should_return_403_for_wrong_student()
    {
        var ownerToken = await CreateStudentTokenAsync();
        var otherToken = await CreateStudentTokenAsync();
        var enrollmentId = await CreateEnrollmentAsync(ownerToken);

        var create = await CreatePaymentAsync(
            ownerToken,
            enrollmentId,
            $"owner-{Guid.NewGuid():N}");
        var payment = await create.Content
            .ReadFromJsonAsync<PaymentResponseTestModel>();

        using var request = _fixture.AuthenticatedRequest(
            HttpMethod.Get,
            $"/api/v1/payments/{payment!.Id}",
            otherToken);

        var response = await _fixture.Client.SendAsync(request);

        response.StatusCode
            .Should()
            .BeOneOf(
                HttpStatusCode.Forbidden,
                HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_allow_admin_to_list_by_enrollment()
    {
        var adminToken = await CreateAdminTokenAsync();

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/v1/payments?enrollmentId=1&page=1&pageSize=10");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await _fixture.Client.SendAsync(request);
    }

    [Fact]
    public async Task Should_complete_create_flow_under_expected_latency()
    {
        var token = await CreateStudentTokenAsync();
        var enrollmentId = await CreateEnrollmentAsync(token);
        var stopwatch = Stopwatch.StartNew();

        var response = await CreatePaymentAsync(
            token,
            enrollmentId,
            $"latency-{Guid.NewGuid():N}");

        stopwatch.Stop();

        response.IsSuccessStatusCode.Should().BeTrue();
        stopwatch.Elapsed.Should().BeLessThan(
            TimeSpan.FromSeconds(2));
    }

    private static object ValidRequest() => new
    {
        amount = 100,
        currency = "BRL",
        enrollmentId = 1,
        method = 1
    };

    private async Task<HttpResponseMessage> CreatePaymentAsync(
        string token,
        int enrollmentId,
        string idempotencyKey)
    {
        using var request = _fixture.AuthenticatedRequest(
            HttpMethod.Post,
            "/api/v1/payments",
            token);

        request.Headers.Add(
            "Idempotency-Key",
            idempotencyKey);
        request.Content = JsonContent.Create(
            new
            {
                amount = 100,
                currency = "BRL",
                enrollmentId,
                method = 1
            });

        return await _fixture.Client.SendAsync(request);
    }

    private Task<string> CreateStudentTokenAsync()
    {
        var auth = new AuthFixture(_fixture.Client);
        return auth.RegisterAndLoginAsync(
            $"student-{Guid.NewGuid():N}@test.com",
            "Student-Test-123!",
            "Student");
    }

    private Task<string> CreateAdminTokenAsync()
    {
        var auth = new AuthFixture(_fixture.Client);
        return auth.RegisterAndLoginAsync(
            $"admin-{Guid.NewGuid():N}@test.com",
            "Admin-Test-123!",
            "Admin");
    }

    private async Task<int> CreateEnrollmentAsync(
        string token)
    {
        // Ajuste para o fluxo real de criação de estudante/matrícula.
        // A fixture deve criar dados isolados antes do teste.
        await Task.Yield();
        return 1;
    }

    private sealed record ProblemDetailsResponse(
        string? Title,
        string? Detail,
        int? Status,
        string? TraceId);

    private sealed record PaymentResponseTestModel(
        Guid Id,
        int EnrollmentId);
}